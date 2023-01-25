import { computed, Ref, ref } from "vue";
import { useLogger, useUser } from "@vc-shell/framework";
import {
  TaskManagementClient,
  IWorkTask,
  WorkTaskSearchCriteria,
  WorkTaskSearchResult,
} from "../../../../api_client/taskmanagement";

interface IUseWorkTasks {
  readonly workTasks: Ref<IWorkTask[]>;
  readonly totalCount: Ref<number>;
  readonly pages: Ref<number>;
  readonly loading: Ref<boolean>;
  readonly currentPage: Ref<number>;
  loadWorkTasks(criteria?: WorkTaskSearchCriteria): void;
}

export default (): IUseWorkTasks => {
  const logger = useLogger();
  const { getAccessToken, user } = useUser();
  const loading = ref(false);
  const workTasks = ref(new WorkTaskSearchResult({ results: [] }));
  const currentPage = ref(1);

  async function getApiClient(): Promise<TaskManagementClient> {
    const client = new TaskManagementClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function loadWorkTasks(criteria?: WorkTaskSearchCriteria) {
    loading.value = true;
    const client = await getApiClient();
    try {
      workTasks.value = await client.searchTasks(criteria);
      currentPage.value =
        (criteria?.skip || 0) / Math.max(1, criteria?.take || 20) + 1;
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    workTasks: computed(() => workTasks.value?.results),
    totalCount: computed(() => workTasks.value?.totalCount),
    pages: computed(() => Math.ceil(workTasks.value?.totalCount / 20)),
    loading: computed(() => loading.value),
    currentPage: computed(() => currentPage.value),
    loadWorkTasks: loadWorkTasks,
  };
};
