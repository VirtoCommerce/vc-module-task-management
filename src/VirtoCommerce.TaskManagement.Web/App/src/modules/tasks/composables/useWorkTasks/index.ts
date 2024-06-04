import { computed, ref, watch } from "vue";
import {
  TaskManagementClient,
  TaskType,
  WorkTask,
  WorkTaskPriority,
  WorkTaskSearchCriteria,
} from "../../../../api_client/virtocommerce.taskmanagement";
import {
  ListBaseBladeScope,
  ListComposableArgs,
  useApiClient,
  useBladeNavigation,
  useListFactory,
} from "@vc-shell/framework";
import useWorkTaskTypes from "../useWorkTaskTypes";

export interface DynamicItemsScope extends ListBaseBladeScope {}

const { getApiClient } = useApiClient(TaskManagementClient);
const { getTaskTypes } = useWorkTaskTypes();

export default (args: ListComposableArgs) => {
  const { mounted } = args;
  const factory = useListFactory<WorkTask[], WorkTaskSearchCriteria>({
    load: async (searchQuery) => {
      const client = await getApiClient();
      return client.searchTasks({
        take: 20,
        ...(searchQuery || {}),
      } as WorkTaskSearchCriteria);
    },
  });

  const taskTypes = ref(<TaskType[]>[]);

  watch(mounted, async () => {
    const result = await getTaskTypes();
    taskTypes.value = result.results || [];
  });

  const { load, items, pagination, loading, query } = factory();
  const { openBlade, resolveBladeByName } = useBladeNavigation();

  async function openDetailsBlade(data?: Omit<Parameters<typeof openBlade>["0"], "blade">) {
    await openBlade({
      blade: resolveBladeByName("WorkTaskDetails"),
      ...data,
    });
  }

  const scope: DynamicItemsScope = {
    openDetailsBlade,
    taskTypes,
    priorities: computed(() =>
      Object.values(WorkTaskPriority).map((value) => ({
        value,
        label: value,
      })),
    ),
    toolbarOverrides: {
      openCreateTaskBlade: {
        clickHandler: async () => {
          await openDetailsBlade();
        },
        isVisible: computed(() => true),
      },
    },
  };

  return {
    items,
    load,
    loading,
    pagination,
    query,
    scope,
  };
};
