import { DynamicBladeList } from "@vc-shell/framework";
import { useWorkTasks } from "../../../tasks/composables";
import { Ref } from "vue";
import { WorkTaskSearchCriteria } from "../../../../api_client/virtocommerce.taskmanagement";

export const useMyWorkTasks = (args: {
  props: InstanceType<typeof DynamicBladeList>["$props"];
  emit: InstanceType<typeof DynamicBladeList>["$emit"];
  mounted: Ref<boolean>;
}) => {
  const { items, load, loading, query, pagination, scope } = useWorkTasks({
    ...args,
    isWidgetView: false // <-- Add this line. Set to `true` if appropriate!
  });

  const loadWrap = async (loadQuery?: WorkTaskSearchCriteria) => {
    query.value = Object.assign(query.value, loadQuery, { onlyAssignedToMe: true });

    await load(query.value);
  };

  return {
    items,
    load: loadWrap,
    loading,
    query,
    pagination,
    scope,
  };
};
