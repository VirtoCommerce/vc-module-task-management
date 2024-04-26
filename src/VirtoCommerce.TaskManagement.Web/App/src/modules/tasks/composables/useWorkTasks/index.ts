import { computed, Ref, ref, watch } from "vue";
import {
  TaskManagementClient,
  TaskType,
  WorkTask,
  WorkTaskPriority,
  WorkTaskSearchCriteria,
} from "../../../../api_client/virtocommerce.taskmanagement";
import {
  DynamicBladeList,
  ListBaseBladeScope,
  useApiClient,
  useBladeNavigation,
  useListFactory,
} from "@vc-shell/framework";
import useWorkTaskTypes from "../useWorkTaskTypes";

export interface DynamicItemsScope extends ListBaseBladeScope {}

const { getApiClient } = useApiClient(TaskManagementClient);
const { getTaskTypes } = useWorkTaskTypes();

export default (args: {
  props: InstanceType<typeof DynamicBladeList>["$props"];
  emit: InstanceType<typeof DynamicBladeList>["$emit"];
  mounted: Ref<boolean>;
}) => {
  const { mounted } = args;
  const factory = useListFactory<WorkTask[], WorkTaskSearchCriteria>({
    load: async (query) => {
      const client = await getApiClient();
      return await client.searchTasks({
        take: 20,
        ...(query || {}),
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

  const scope = ref<DynamicItemsScope>({
    openDetailsBlade,
    taskTypes,
    priorities: computed(() =>
      Object.values(WorkTaskPriority).map((value) => ({
        value,
        label: value,
      })),
    ),
  });

  return {
    items,
    load,
    loading,
    pagination,
    query,
    scope: computed(() => scope.value),
  };
};
