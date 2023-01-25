import { computed, Ref, ref } from "vue";
import { useLogger, useUser } from "@vc-shell/framework";
import {
  TaskManagementClient,
  WorkTask,
} from "../../../../api_client/taskmanagement";

interface IUseWorkTask {
  workTask: Ref<WorkTask>;
  newWorkTask: Ref<WorkTask>;
  loading: Ref<boolean>;
  loadWorkTask(id: string): void;
  approveWorkTask(id: string, result: any): void;
  rejectWorkTask(id: string, result: any): void;
  createWorkTask(workTask: WorkTask): void;
}

const workTask: Ref<WorkTask> = ref({} as WorkTask);
const newWorkTask: Ref<WorkTask> = ref({} as WorkTask);

export default (): IUseWorkTask => {
  const logger = useLogger();
  const loading = ref(false);

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

  async function createWorkTask(): Promise<void> {
    loading.value = true;
    const client = await getApiClient();
    try {
      loading.value = true;
      workTask.value = await client.create(newWorkTask.value);
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    workTask: computed(() => workTask.value),
    newWorkTask: computed(() => newWorkTask.value),
    loading,
    loadWorkTask,
    approveWorkTask,
    rejectWorkTask,
    createWorkTask,
  };
};
