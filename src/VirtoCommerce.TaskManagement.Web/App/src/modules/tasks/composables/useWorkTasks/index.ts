import { computed, ref, onMounted, Ref, ComputedRef } from "vue";
import {
  IWorkTaskSearchCriteria,
  TaskManagementClient,
  TaskType,
  WorkTask,
  WorkTaskPriority,
  WorkTaskSearchCriteria,
  WorkTaskSearchResult,
} from "../../../../api_client/virtocommerce.taskmanagement";
import { useApiClient, useAsync, useLoading } from "@vc-shell/framework";
import useWorkTaskTypes from "../useWorkTaskTypes";

export interface UseBaseWorkTasksListOptions {
  pageSize?: number;
  sort?: string;
  defaultFilters?: Partial<IWorkTaskSearchCriteria>;
}

export interface IUseBaseWorkTasksList {
  items: ComputedRef<WorkTask[]>;
  totalCount: ComputedRef<number>;
  pages: ComputedRef<number>;
  currentPage: ComputedRef<number>;
  searchQuery: ComputedRef<IWorkTaskSearchCriteria>;
  loadWorkTasks: (query?: IWorkTaskSearchCriteria) => Promise<void>;
  loading: ComputedRef<boolean>;
  taskTypes: Ref<TaskType[]>;
  priorities: ComputedRef<{ value: WorkTaskPriority; label: string }[]>;
}

export function useWorkTasksList(options?: UseBaseWorkTasksListOptions): IUseBaseWorkTasksList {
  const { getApiClient } = useApiClient(TaskManagementClient);
  const { getTaskTypes } = useWorkTaskTypes();

  const pageSize = options?.pageSize || 20;
  const searchQuery = ref<WorkTaskSearchCriteria>(
    new WorkTaskSearchCriteria({
      take: pageSize,
      sort: options?.sort || "modifiedDate:desc",
      skip: 0,
      ...options?.defaultFilters,
    }),
  );

  const searchResult = ref<WorkTaskSearchResult>();
  const taskTypes = ref<TaskType[]>([]);

  const { action: loadWorkTasks, loading: loadingWorkTasks } = useAsync<IWorkTaskSearchCriteria>(async (_query) => {
    // Merge with existing query and apply filters
    searchQuery.value = new WorkTaskSearchCriteria({
      ...searchQuery.value,
      ...(_query || {}),
      ...options?.defaultFilters, // Always apply default filters
    });

    const client = await getApiClient();
    searchResult.value = await client.searchTasks(searchQuery.value);
  });

  onMounted(async () => {
    console.warn("useWorkTasks (list) mounted - begin");
    const typesResult = await getTaskTypes();
    taskTypes.value = typesResult.results || [];
    console.warn("useWorkTasks (list) mounted - end", taskTypes.value);
  });

  return {
    items: computed(() => searchResult.value?.results || []),
    totalCount: computed(() => searchResult.value?.totalCount || 0),
    pages: computed(() => Math.ceil((searchResult.value?.totalCount || 1) / pageSize)),
    currentPage: computed(() => Math.ceil((searchQuery.value?.skip || 0) / Math.max(1, pageSize) + 1)),
    searchQuery: computed(() => searchQuery.value),
    loadWorkTasks,
    loading: useLoading(loadingWorkTasks),
    taskTypes,
    priorities: computed(() =>
      Object.values(WorkTaskPriority).map((value) => ({
        value,
        label: value as string,
      })),
    ),
  };
}
