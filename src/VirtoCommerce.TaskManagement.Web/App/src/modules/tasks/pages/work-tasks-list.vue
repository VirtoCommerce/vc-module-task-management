<template>
  <TasksList
    ref="tasksList"
    :expanded="props.expanded"
    :param="props.param"
    :title="$t('TASKS.PAGES.LIST.ALL_TASKS_TITLE')"
    :is-current-user-list="false"
    :only-completed-list="false"
  ></TasksList>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import TasksList from "../components/tasksList.vue";
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

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

defineOptions({
  url: "/active",
});

const tasksList = ref(null);

const reload = async () => {
  await tasksList.value.reload();
};

defineExpose({
  reload,
});
</script>
