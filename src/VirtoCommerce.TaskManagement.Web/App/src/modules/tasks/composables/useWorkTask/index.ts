import { computed, Ref, ref, watch } from "vue";
import { useLogger, useUser } from "@vc-shell/framework";
import {
  TaskManagementClient,
  WorkTask,
  WorkTaskPriority,
} from "../../../../api_client/taskmanagement";
import { cloneDeep, isEqual } from "lodash-es";

interface IUseWorkTask {
  workTask: Ref<WorkTask>;
  loading: Ref<boolean>;
  modified: Ref<boolean>;
  priorities: WorkTaskPriority[];
  loadWorkTask(id: string): void;
  createWorkTask(workTask: WorkTask): void;
  approveWorkTask(id: string, result: any): void;
  rejectWorkTask(id: string, result: any): void;
  updateWorktask(): void;
  deleteWorkTask(id: string): void;
}

const workTask: Ref<WorkTask> = ref({} as WorkTask);

export default (): IUseWorkTask => {
  const logger = useLogger();
  const loading = ref(false);
  const priorities = Object.values(WorkTaskPriority);

  let workTaskCopy: WorkTask;

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

  async function loadWorkTask(id: string): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.get(id);
      workTaskCopy = cloneDeep(workTask.value);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function createWorkTask(newWorkTask: WorkTask): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.create(newWorkTask);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function approveWorkTask(id: string, result: any): Promise<void> {
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.complete(id, result);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function rejectWorkTask(id: string, result: any): Promise<void> {
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.cancel(id, result);
    } catch (e) {
      logger.error(e);
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
      workTask.value = await client.update(workTask.value);
      workTaskCopy = cloneDeep(workTask.value);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function deleteWorkTask(id: string): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      await client.delete(id);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    workTask: computed(() => workTask.value),
    loading: computed(() => loading.value),
    modified: computed(() => modified.value),
    priorities,
    loadWorkTask,
    createWorkTask,
    approveWorkTask,
    rejectWorkTask,
    updateWorktask,
    deleteWorkTask,
  };
};
