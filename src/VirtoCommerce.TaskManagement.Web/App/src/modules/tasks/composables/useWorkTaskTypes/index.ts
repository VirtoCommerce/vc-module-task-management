import { computed, Ref, ref } from "vue";
import { useUser } from "@vc-shell/framework";
import {
  TaskManagementClient, 
  TaskType,
} from "../../../../api_client/taskmanagement";

interface IWorkTaskTypeResult {
  totalCount?: number;
  results?: TaskType[];
}

interface IUseWorkTaskTypes {
  readonly loading: Ref<boolean>;
  getTaskTypes(): Promise<IWorkTaskTypeResult>;
}

export default (): IUseWorkTaskTypes => {
  const loading = ref(false);

  async function getApiClient(): Promise<TaskManagementClient> {
    const { getAccessToken } = useUser();
    const client = new TaskManagementClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function getTaskTypes(): Promise<IWorkTaskTypeResult> {
    loading.value = true;
    const client = await getApiClient();
    try {
      let types = await client.getTaskTypes();
      return {
        totalCount: types.length,
        results: types,
      };
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    loading: computed(() => loading.value),
    getTaskTypes,
  };
};
