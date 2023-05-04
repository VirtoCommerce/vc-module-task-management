import { computed, Ref, ref, watch } from "vue";
import { useUser } from "@vc-shell/framework";
import {
  TaskManagementClient,
  WorkTask,
  WorkTaskPriority,
} from "../../../../api_client/taskmanagement";
import { cloneDeep, forEach, isEqual } from "lodash-es";

interface IUseWorkTask {
  workTask: Ref<WorkTask>;
  loading: Ref<boolean>;
  taskAvailable: Ref<boolean>;
  modified: Ref<boolean>;
  priorities: WorkTaskPriority[];
  initNewWorkTask(): void;
  loadWorkTask(id: string): Promise<void>;
  createWorkTask(): Promise<void>;
  approveWorkTask(id: string, result: any): Promise<void>;
  rejectWorkTask(id: string, result: any): Promise<void>;
  updateWorktask(): Promise<void>;
  resetWorkTask(): void;
  deleteWorkTask(id: string): Promise<void>;
}

const workTask: Ref<WorkTask> = ref({
  priority: WorkTaskPriority.Normal,
  attachments: [],
  isActive: true,
} as WorkTask);

export default (): IUseWorkTask => {
  const loading = ref(false);
  const taskAvailable = ref(false);
  const priorities = Object.values(WorkTaskPriority);

  let workTaskCopy: WorkTask;
  const newWorkTask = ref({
    priority: WorkTaskPriority.Normal,
    attachments: [],
    isActive: true,
  } as WorkTask);

  const modified = ref(false);

  watch(
    () => workTask,
    (state) => {
      modified.value = !isEqual(workTaskCopy, state.value);
    },
    { deep: true }
  );

  async function getApiClient(): Promise<TaskManagementClient> {
    const { getAccessToken } = useUser();
    const client = new TaskManagementClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  function initNewWorkTask(): void {
    workTask.value = newWorkTask.value;
  }

  async function loadWorkTask(id: string): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.get(id, "WithAttachments");
      workTaskCopy = cloneDeep(workTask.value);
      taskAvailable.value = workTask.value.id !== undefined;
    } catch (e) {
      taskAvailable.value = false;
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function createWorkTask(): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      validateAttachments(workTask.value);
      workTask.value = await client.create(workTask.value);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function approveWorkTask(id: string, result: any): Promise<void> {
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.finish(id, true, result);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function rejectWorkTask(id: string, result: any): Promise<void> {
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.finish(id, false, result);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function updateWorktask(): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      validateAttachments(workTask.value);
      workTask.value = await client.update(workTask.value);
      workTaskCopy = cloneDeep(workTask.value);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  function resetWorkTask(): void {
    workTask.value = cloneDeep(workTaskCopy);
  }

  async function deleteWorkTask(id: string): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      await client.delete(id);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  function validateAttachments(task: WorkTask) {
    forEach(task.attachments, function (attachment) {
      if (attachment.id && attachment.id.startsWith("Draft")) {
        attachment.id = null;
      }
    });
  }

  return {
    workTask: computed(() => workTask.value),
    loading: computed(() => loading.value),
    taskAvailable: computed(() => taskAvailable.value),
    modified: computed(() => modified.value),
    priorities,
    initNewWorkTask,
    loadWorkTask,
    createWorkTask,
    approveWorkTask,
    rejectWorkTask,
    updateWorktask,
    resetWorkTask,
    deleteWorkTask,
  };
};
