import { computed, ref, onMounted, Ref, ComputedRef } from "vue";
import {
  TaskManagementClient,
  TaskPriority,
  TaskType,
  WorkTask,
  WorkTaskSearchCriteria,
  WorkTaskSearchResult,
} from "../../../../api_client/virtocommerce.taskmanagement";
import { useApiClient, useAsync, useDataTablePagination, useLoading } from "@vc-shell/framework";
import useWorkTaskTypes from "../useWorkTaskTypes";

export interface UseBaseWorkTasksListOptions {
  pageSize?: number;
  sort?: string;
  defaultFilters?: Partial<WorkTaskSearchCriteria>;
}

export interface IUseBaseWorkTasksList {
  items: ComputedRef<WorkTask[]>;
  pagination: ReturnType<typeof useDataTablePagination>;
  searchQuery: ComputedRef<WorkTaskSearchCriteria>;
  loadWorkTasks: (query?: WorkTaskSearchCriteria) => Promise<void>;
  loading: ComputedRef<boolean>;
  taskTypes: Ref<TaskType[]>;
  priorities: ComputedRef<{ value: TaskPriority; label: string }[]>;
}

export function useWorkTasksList(options?: UseBaseWorkTasksListOptions): IUseBaseWorkTasksList {
  const { getApiClient } = useApiClient(TaskManagementClient);
  const { getTaskTypes } = useWorkTaskTypes();

  const pageSize = options?.pageSize || 20;
  const searchQuery = ref<WorkTaskSearchCriteria>({
    take: pageSize,
    sort: options?.sort || "modifiedDate:desc",
    skip: 0,
    ...options?.defaultFilters,
  } as WorkTaskSearchCriteria);

  const searchResult = ref<WorkTaskSearchResult>();
  const taskTypes = ref<TaskType[]>([]);

  const { action: loadWorkTasks, loading: loadingWorkTasks } = useAsync<WorkTaskSearchCriteria>(async (_query) => {
    searchQuery.value = {
      ...searchQuery.value,
      ...(_query || {}),
      ...options?.defaultFilters,
    } as WorkTaskSearchCriteria;

    const client = await getApiClient();
    searchResult.value = await client.searchTasks(searchQuery.value);
  });

  const pagination = useDataTablePagination({
    pageSize,
    totalCount: computed(() => searchResult.value?.totalCount || 0),
    onPageChange: ({ skip }) => {
      void loadWorkTasks({ ...searchQuery.value, skip } as WorkTaskSearchCriteria);
    },
  });

  onMounted(async () => {
    const typesResult = await getTaskTypes();
    taskTypes.value = typesResult.results || [];
  });

  return {
    items: computed(() => searchResult.value?.results || []),
    pagination,
    searchQuery: computed(() => searchQuery.value),
    loadWorkTasks,
    loading: useLoading(loadingWorkTasks),
    taskTypes,
    priorities: computed(() =>
      Object.values(TaskPriority).map((value) => ({
        value,
        label: value as string,
      })),
    ),
  };
}
