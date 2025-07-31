import { computed, reactive, ref, Ref, ComputedRef } from "vue";
import {
  MemberSearchResult,
  TaskManagementClient,
  WorkTask,
  WorkTaskPriority,
} from "../../../../api_client/virtocommerce.taskmanagement";
import { useApiClient, useAsync, useLoading, useModificationTracker } from "@vc-shell/framework";

import useWorkTaskTypes, { IWorkTaskTypeResult } from "../useWorkTaskTypes";
import useContacts from "../useContacts";

export interface IPriority {
  name: string;
  value: WorkTaskPriority;
}

export interface IUseWorkTaskDetails {
  item: Ref<WorkTask>;
  isModified: Readonly<Ref<boolean>>;
  loading: ComputedRef<boolean>;
  loadWorkTask: () => Promise<void>;
  saveWorkTask: (status?: string) => Promise<WorkTask>;
  completeWorkTask: () => Promise<WorkTask>;
  rejectWorkTask: () => Promise<WorkTask>;
  deleteWorkTask: () => Promise<void>;
  resetWorkTask: () => void;
  isReadOnly: ComputedRef<boolean>;
  priorities: ComputedRef<IPriority[]>;
  loadTaskTypes: () => Promise<IWorkTaskTypeResult>;
  searchContacts: (keyword?: string, skip?: number, ids?: string[]) => Promise<MemberSearchResult>;
  statusText: ComputedRef<string>;
}

export interface UseWorkTaskDetailsOptions {
  id?: string;
  isNew?: boolean;
}

export function useWorkTaskDetails(options?: UseWorkTaskDetailsOptions): IUseWorkTaskDetails {
  const { getTaskTypes } = useWorkTaskTypes();
  const { searchContacts, getMember } = useContacts();
  const apiClient = useApiClient(TaskManagementClient);

  const item = ref<WorkTask>(
    reactive(
      new WorkTask({
        priority: WorkTaskPriority.Normal,
        attachments: [],
        isActive: true,
      }),
    ),
  );

  const { currentValue, isModified, resetModificationState } = useModificationTracker(item);

  const { action: loadWorkTask, loading: loadingWorkTask } = useAsync(async () => {
    if (options?.id && !options?.isNew) {
      const client = await apiClient.getApiClient();
      const result = await client.get(options.id, "WithAttachments");
      currentValue.value = reactive(result);
      resetModificationState();
    } else if (options?.isNew || !options?.id) {
      currentValue.value = reactive(
        new WorkTask({
          priority: WorkTaskPriority.Normal,
          attachments: [],
          isActive: true,
        }),
      );
      resetModificationState();
    }
  });

  const { action: saveWorkTask, loading: savingWorkTask } = useAsync(async (status?: string) => {
    const client = await apiClient.getApiClient();

    if (status) {
      currentValue.value.status = status;
    }

    if (currentValue.value.responsibleId) {
      const contact = await getMember(currentValue.value.responsibleId);
      if (contact) {
        currentValue.value.responsibleName = contact.name;
      }
    }

    const result = await client.create(currentValue.value);
    currentValue.value = reactive(result);
    resetModificationState();
    return result;
  });

  const { action: completeWorkTask, loading: completingWorkTask } = useAsync(async () => {
    if (currentValue.value?.id) {
      const client = await apiClient.getApiClient();
      const result = await client.finish(currentValue.value.id, true, currentValue.value.parameters);
      currentValue.value = reactive(result);
      resetModificationState();
      return result;
    }
    throw new Error("Task ID is required");
  });

  const { action: rejectWorkTask, loading: rejectingWorkTask } = useAsync(async () => {
    if (currentValue.value?.id) {
      const client = await apiClient.getApiClient();
      const result = await client.finish(currentValue.value.id, false, currentValue.value.parameters);
      currentValue.value = reactive(result);
      resetModificationState();
      return result;
    }
    throw new Error("Task ID is required");
  });

  const { action: deleteWorkTask, loading: deletingWorkTask } = useAsync(async () => {
    if (currentValue.value?.id) {
      const client = await apiClient.getApiClient();
      await client.delete(currentValue.value.id);
    }
  });

  const resetWorkTask = () => {
    resetModificationState();
  };

  const isReadOnly = computed(() => {
    return (currentValue.value != null && !currentValue.value.isActive) || false;
  });

  const priorities = computed(() =>
    Object.values(WorkTaskPriority).map((priority) => ({
      name: priority,
      value: priority,
    })),
  );

  const loadTaskTypes = async () => {
    return getTaskTypes();
  };

  const statusText = computed(() => {
    let result = "To Do";
    const workTask = currentValue.value;
    if (workTask == null) {
      return result;
    }
    if (workTask.isActive === true) {
      switch (workTask.completed) {
        case false:
        case true:
          result = "Canceled";
          break;
      }
    } else {
      switch (workTask.completed) {
        case null:
        case false:
          result = "Canceled";
          break;
        case true:
          result = "Done";
          break;
      }
    }
    return result;
  });

  return {
    item: currentValue,
    isModified,
    loading: useLoading(loadingWorkTask, savingWorkTask, completingWorkTask, rejectingWorkTask, deletingWorkTask),
    loadWorkTask,
    saveWorkTask,
    completeWorkTask,
    rejectWorkTask,
    deleteWorkTask,
    resetWorkTask,
    isReadOnly,
    priorities,
    loadTaskTypes,
    searchContacts,
    statusText,
  };
}
