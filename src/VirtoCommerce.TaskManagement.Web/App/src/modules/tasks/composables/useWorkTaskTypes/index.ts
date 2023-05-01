import { computed, Ref, ref } from "vue";
import { useUser } from "@vc-shell/framework";
import {
  TaskManagementClient,
  TaskType,
} from "../../../../api_client/taskmanagement";
import { orderBy, sortBy } from "lodash-es";

interface IWorkTaskTypeResult {
  totalCount?: number;
  results?: TaskType[];
}

interface IUseWorkTaskTypes {
  readonly loading: Ref<boolean>;
  readonly types: Ref<TaskType[]>;
  getTaskTypes(): Promise<IWorkTaskTypeResult>;
}

export default (): IUseWorkTaskTypes => {
  const loading = ref(false);
  const types = ref<TaskType[]>([]);

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
      types.value = await client.getTaskTypes();
      types.value = orderBy(types.value, ["name"], ["asc"]);
      types.value = sortBy(types.value, (type) =>
        type.name === "Other" || type.name === "Others" ? 1 : 0
      );
      return {
        totalCount: types.value.length,
        results: types.value,
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
    types: computed(() => types.value),
    getTaskTypes,
  };
};
