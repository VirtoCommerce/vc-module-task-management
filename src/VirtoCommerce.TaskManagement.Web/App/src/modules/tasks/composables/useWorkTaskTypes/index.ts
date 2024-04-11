import { computed, Ref, ref } from "vue";
import { TaskManagementClient, TaskType } from "../../../../api_client/virtocommerce.taskmanagement";
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
    const client = new TaskManagementClient();
    return client;
  }

  async function getTaskTypes(): Promise<IWorkTaskTypeResult> {
    loading.value = true;
    const client = await getApiClient();
    try {
      const loadedTypes = await client.getTaskTypes();
      const orderedTypes = orderBy(loadedTypes, ["name"], ["asc"]);
      const result = sortBy(orderedTypes, (type) => (type.name === "Other" || type.name === "Others" ? 1 : 0));
      types.value = result;
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
