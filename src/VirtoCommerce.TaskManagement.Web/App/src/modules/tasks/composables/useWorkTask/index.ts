import { computed, reactive, Ref, ref, watch } from "vue";
import { TaskManagementClient, WorkTask, WorkTaskPriority } from "../../../../api_client/virtocommerce.taskmanagement";
import {
  DetailsBaseBladeScope,
  DynamicBladeForm,
  IBladeToolbar,
  useApiClient,
  useAssets,
  useDetailsFactory,
  usePermissions,
} from "@vc-shell/framework";

import useWorkTaskTypes from "../useWorkTaskTypes";
import useContacts from "../useContacts";
import { TaskPermissions } from "../../../../types";

const { getTaskTypes } = useWorkTaskTypes();
const { searchContacts } = useContacts();

export interface WorkTaskDetailScope extends DetailsBaseBladeScope {
  toolbarOverrides: {
    saveChanges: IBladeToolbar;
    completeWorkTask: IBladeToolbar;
    rejectWorkTask: IBladeToolbar;
    removeWorkTask: IBladeToolbar;
    resetWorkTask: IBladeToolbar;
  };
}

const { getApiClient } = useApiClient(TaskManagementClient);

export default (args: {
  props: InstanceType<typeof DynamicBladeForm>["$props"];
  emit: InstanceType<typeof DynamicBladeForm>["$emit"];
  mounted: Ref<boolean>;
}) => {
  const factory = useDetailsFactory<WorkTask>({
    load: async (item) => {
      return await (await getApiClient()).get(item!.id, "WithAttachments");
    },
    saveChanges: async (item) => {
      return await (await getApiClient()).create(item);
    },
    remove: async (item) => {
      (await getApiClient()).delete(item.id);
    },
  });

  const { load, saveChanges, remove: deleteWorkTask, loading, item, validationState } = factory();
  const { upload: imageUpload, remove: imageRemove, edit: imageEdit, loading: imageLoading } = useAssets();
  const { hasAccess } = usePermissions();

  const scope = ref<WorkTaskDetailScope>({
    toolbarOverrides: {
      saveChanges: {
        isVisible: computed(
          () =>
            (!!args.props.param && item.value?.isActive && hasAccess(TaskPermissions.Update)) ||
            (!args.props.param && item.value?.isActive && hasAccess(TaskPermissions.Create)),
        ),
        disabled: computed(() => !validationState.value.modified || !validationState.value.valid),
      },
      completeWorkTask: {
        clickHandler: async () => {
          if (item.value?.id) {
            item.value = await (await getApiClient()).finish(item.value.id, true, item.value.parameters);
          }
        },
        isVisible: computed(() => !!args.props.param && item.value?.isActive && hasAccess(TaskPermissions.Finish)),
      },
      rejectWorkTask: {
        clickHandler: async () => {
          if (item.value?.id) {
            item.value = await (await getApiClient()).finish(item.value.id, false, item.value.parameters);
          }
        },
        isVisible: computed(() => !!args.props.param && item.value?.isActive && hasAccess(TaskPermissions.Finish)),
      },
      removeWorkTask: {
        async clickHandler() {
          if (item.value?.id) {
            const itemId = { id: item.value.id };
            await deleteWorkTask!(itemId);
            args.emit("parent:call", { method: "reload" });
            args.emit("close:blade");
          }
        },
        isVisible: computed(() => !!args.props.param && hasAccess(TaskPermissions.Delete)),
      },
      resetWorkTask: {
        clickHandler: () => {
          validationState.value.resetModified(item, true);
        },
        disabled: computed(() => !validationState.value.modified),
        isVisible: computed(() => !!args.props.param && item.value?.isActive && hasAccess(TaskPermissions.Update)),
      },
    },
    summaryVisibility: computed(() => !args.props.param),
    priorities: Object.values(WorkTaskPriority),
    loadTaskTypes: async () => {
      return await getTaskTypes();
    },
    searchContacts: searchContacts,
    statusText: computed(() => {
      let result = "ToDo";
      const workTask = item.value;
      if (workTask == null) {
        return result;
      }
      if (workTask.isActive === true) {
        switch (workTask.completed) {
          case false:
          case true:
            result = "Canceled";
            break;
        }
      } else {
        switch (workTask.completed) {
          case null:
          case false:
            result = "Canceled";
            break;
          case true:
            result = "Done";
            break;
        }
      }
      return result;
    }),
    assetsHandler: {
      images: {
        loading: imageLoading,
        async upload(files, startingSortOrder) {
          return (await imageUpload(files, `tasks/${item.value?.id}`, startingSortOrder)).map((x) => {
            const result = new Image(x.size);
            console.log(result);
            return result;
          });
        },
        remove(files) {
          return imageRemove(files, item.value?.attachments ?? []);
        },
        edit(files) {
          return imageEdit(files, item.value?.attachments ?? []).map((x) => new Image(x.size));
        },
      },
    },
  });

  const bladeTitle = computed(() => {
    return args.props.param ? "# " + item.value?.number + ": " + item.value?.name : "TASKS.PAGES.DETAILS.NEW_TITLE";
  });

  watch(
    () => args?.mounted.value,
    async () => {
      if (!args.props.param) {
        item.value = reactive(
          new WorkTask({
            priority: WorkTaskPriority.Normal,
            attachments: [],
            isActive: true,
          }),
        );
        validationState.value.resetModified(item.value, true);
      }
    },
  );

  return {
    load,
    saveChanges,
    deleteWorkTask,
    loading,
    item,
    validationState,
    bladeTitle,
    scope: computed(() => scope.value),
  };

  // const taskAvailable = ref(false);
  // const priorities = Object.values(WorkTaskPriority);

  // let workTaskCopy: WorkTask;
  // const newWorkTask = ref({
  //   priority: WorkTaskPriority.Normal,
  //   attachments: [],
  //   isActive: true,
  // } as unknown as WorkTask);

  // const modified = ref(false);

  // watch(
  //   () => workTask,
  //   (state) => {
  //     modified.value = !isEqual(workTaskCopy, state.value);
  //   },
  //   { deep: true },
  // );

  // function initNewWorkTask(): void {
  //   workTask.value = newWorkTask.value;
  // }

  // async function loadWorkTask(id: string): Promise<void> {
  //   loading.value = true;
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     workTask.value = await client.get(id, "WithAttachments");
  //     workTaskCopy = cloneDeep(workTask.value);
  //     taskAvailable.value = workTask.value.id !== undefined;
  //   } catch (e) {
  //     taskAvailable.value = false;
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // async function createWorkTask(): Promise<void> {
  //   loading.value = true;
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     validateAttachments(workTask.value);
  //     workTask.value = await client.create(workTask.value);
  //   } catch (e) {
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // async function approveWorkTask(id: string, result: unknown): Promise<void> {
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     workTask.value = await client.finish(id, true, result);
  //   } catch (e) {
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // async function rejectWorkTask(id: string, result: unknown): Promise<void> {
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     workTask.value = await client.finish(id, false, result);
  //   } catch (e) {
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // async function updateWorktask(): Promise<void> {
  //   loading.value = true;
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     validateAttachments(workTask.value);
  //     workTask.value = await client.update(workTask.value);
  //     workTaskCopy = cloneDeep(workTask.value);
  //   } catch (e) {
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // function resetWorkTask(): void {
  //   workTask.value = cloneDeep(workTaskCopy);
  // }

  // async function deleteWorkTask(id: string): Promise<void> {
  //   loading.value = true;
  //   const client = await getApiClient();
  //   try {
  //     loading.value = true;
  //     await client.delete(id);
  //   } catch (e) {
  //     console.error(e);
  //     throw e;
  //   } finally {
  //     loading.value = false;
  //   }
  // }

  // function validateAttachments(task: WorkTask) {
  //   forEach(task.attachments, function (attachment) {
  //     if (attachment.id && attachment.id.startsWith("Draft")) {
  //       attachment.id = undefined;
  //     }
  //   });
  // }

  // return {
  //   workTask: computed(() => workTask.value),
  //   loading: computed(() => loading.value),
  //   taskAvailable: computed(() => taskAvailable.value),
  //   modified: computed(() => modified.value),
  //   priorities,
  //   initNewWorkTask,
  //   loadWorkTask,
  //   createWorkTask,
  //   approveWorkTask,
  //   rejectWorkTask,
  //   updateWorktask,
  //   resetWorkTask,
  //   deleteWorkTask,
  // };
};
