import { DynamicBladeList } from "@vc-shell/framework";
import { useWorkTasks } from "../../../tasks/composables";
import { Ref } from "vue";
import { WorkTaskSearchCriteria } from "../../../../api_client/virtocommerce.taskmanagement";

export const useArchiveWorkTasks = (args: {
  props: InstanceType<typeof DynamicBladeList>["$props"];
  emit: InstanceType<typeof DynamicBladeList>["$emit"];
  mounted: Ref<boolean>;
}) => {
  const { items, load, loading, query, pagination, scope } = useWorkTasks({
    props: args.props,
    emit: args.emit,
    mounted: args.mounted,
    isWidgetView: false
  });

  const loadWrap = async (loadQuery?: WorkTaskSearchCriteria) => {
    query.value = Object.assign(query.value, loadQuery, { isActive: false });

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
