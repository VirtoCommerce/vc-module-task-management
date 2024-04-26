import { computed, reactive, Ref, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { TaskManagementClient, WorkTask, WorkTaskPriority } from "../../../../api_client/virtocommerce.taskmanagement";
import {
  DetailsBaseBladeScope,
  DynamicBladeForm,
  IBladeToolbar,
  useApiClient,
  // useAssets,
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
    load: async (loadItem) => {
      const client = await getApiClient();
      return client.get(loadItem!.id, "WithAttachments");
    },
    saveChanges: async (saveItem) => {
      const client = await getApiClient();
      return client.create(saveItem);
    },
    remove: async (removeItem) => {
      (await getApiClient()).delete(removeItem.id);
    },
  });

  const { load, saveChanges, remove: deleteWorkTask, loading, item, validationState } = factory();
  const { hasAccess } = usePermissions();
  const { t } = useI18n();

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
  });

  const bladeTitle = computed(() => {
    return args.props.param ? "# " + item.value?.number + ": " + item.value?.name : t("TASKS.PAGES.DETAILS.NEW_TITLE");
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
};
