import {
  useWorkTasksList,
  IUseBaseWorkTasksList,
  UseBaseWorkTasksListOptions,
} from "../../../tasks/composables/useWorkTasks";

export interface UseMyWorkTasksOptions extends UseBaseWorkTasksListOptions {}

export function useMyWorkTasks(options?: UseMyWorkTasksOptions): IUseBaseWorkTasksList {
  return useWorkTasksList({
    pageSize: options?.pageSize || 20,
    sort: options?.sort || "createdDate:DESC",
    defaultFilters: {
      onlyAssignedToMe: true,
    },
  });
}
