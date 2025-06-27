<template>
  <BaseWorkTasksList
    v-bind="$props"
    ref="baseWorkTasksListRef"
    :title="title"
    state-key="work_tasks_list"
    :items="items"
    :total-count="totalCount"
    :pages="pages"
    :current-page="currentPage"
    :loading="loading"
    :task-types="taskTypes"
    :priorities="priorities"
    :search-query="searchQuery"
    :load-work-tasks="loadWorkTasks"
    @parent:call="$emit('parent:call', $event)"
    @close:blade="$emit('close:blade')"
    @collapse:blade="$emit('collapse:blade')"
    @expand:blade="$emit('expand:blade')"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, ref, toRefs, useTemplateRef } from "vue";
import { useI18n } from "vue-i18n";
import { IParentCallArgs } from "@vc-shell/framework";
import { useWorkTasksList } from "../composables/useWorkTasks";
import BaseWorkTasksList from "../components/BaseWorkTasksList.vue";
import { WorkTask } from "../../../api_client/virtocommerce.taskmanagement";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
  options?: unknown;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "close:blade"): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
}

defineProps<Props>();
defineEmits<Emits>();

defineOptions({
  name: "WorkTasksList",
  url: "/active",
  isWorkspace: true,
  menuItem: {
    title: "TASKS.MENU.TITLE",
    icon: "material-description",
    priority: 1,
  },
});

const { t } = useI18n({ useScope: "global" });

const baseWorkTasksListRef = useTemplateRef("baseWorkTasksListRef");

const { items, totalCount, pages, currentPage, searchQuery, loadWorkTasks, loading, taskTypes, priorities } =
  useWorkTasksList({
    pageSize: 20,
    sort: "name:desc",
  });

const title = computed(() => t("TASKS.PAGES.LIST.TITLE"));

const reload = () => {
  baseWorkTasksListRef.value?.reload();
};
const onItemClick = (item: WorkTask) => {
  baseWorkTasksListRef.value?.onItemClick(item);
};

// Lifecycle
onMounted(async () => {
  await loadWorkTasks();
});

// Expose methods for external API
defineExpose({
  title,
  reload,
  onItemClick,
});
</script> 