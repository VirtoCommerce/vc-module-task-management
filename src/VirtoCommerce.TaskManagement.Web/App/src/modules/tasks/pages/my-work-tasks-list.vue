<template>
  <TasksList
    ref="tasksList"
    :expanded="props.expanded"
    :param="props.param"
    :title="$t('TASKS.PAGES.LIST.MY_ACTIVE_TASKS_TITLE')"
    :is-current-user-list="true"
    :only-completed-list="false"
  ></TasksList>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import TasksList from "../components/tasksList.vue";
import { TaskPermissions } from "../../../types";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "close:blade"): void;
}

defineOptions({
  url: "/my",
  permissions: [TaskPermissions.Access, TaskPermissions.Read],
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

const tasksList = ref(null);

const reload = async () => {
  await tasksList.value.reload();
};

defineExpose({
  reload,
});
</script>
