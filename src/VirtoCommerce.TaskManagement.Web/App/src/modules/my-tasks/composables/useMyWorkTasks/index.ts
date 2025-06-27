import {
  useWorkTasksList,
  IUseBaseWorkTasksList,
  UseBaseWorkTasksListOptions,
} from "../../../tasks/composables/useWorkTasks";

export interface UseMyWorkTasksOptions extends UseBaseWorkTasksListOptions {}

export function useMyWorkTasks(options?: UseMyWorkTasksOptions): IUseBaseWorkTasksList {
  return useWorkTasksList({
    pageSize: options?.pageSize || 20,
    sort: options?.sort || "modifiedDate:desc",
    defaultFilters: {
      onlyAssignedToMe: true,
    },
  });
}
