import {
  useWorkTasksList,
  IUseBaseWorkTasksList,
  UseBaseWorkTasksListOptions,
} from "../../../tasks/composables/useWorkTasks";

export interface UseArchiveWorkTasksOptions extends UseBaseWorkTasksListOptions {}

export function useArchiveWorkTasks(options?: UseArchiveWorkTasksOptions): IUseBaseWorkTasksList {
  return useWorkTasksList({
    pageSize: options?.pageSize || 20,
    sort: options?.sort || "modifiedDate:desc",
    defaultFilters: {
      isActive: false,
    },
  });
}
