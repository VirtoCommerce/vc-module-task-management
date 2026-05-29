<template>
  <BaseWorkTasksList
    v-bind="$props"
    ref="baseWorkTasksListRef"
    :title="title"
    state-key="my_tasks_list"
    :items="items"
    :pagination="pagination"
    :loading="loading"
    :task-types="taskTypes"
    :priorities="priorities"
    :search-query="searchQuery"
    :load-work-tasks="loadWorkTasks"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, useTemplateRef } from "vue";
import { useI18n } from "vue-i18n";
import { useMyWorkTasks } from "../composables/useMyWorkTasks";
import { BaseWorkTasksList } from "../../tasks/components";
import { WorkTask } from "../../../api_client/virtocommerce.taskmanagement";
import { useBlade } from "@vc-shell/framework";

const { exposeToChildren } = useBlade();

defineBlade({
  name: "MyTasksList",
  url: "/my",
  isWorkspace: true,
  menuItem: {
    title: "MY_TASKS.MENU.TITLE",
    icon: "lucide-file-text",
    priority: 1,
  },
});

const { t } = useI18n({ useScope: "global" });

const baseWorkTasksListRef = useTemplateRef("baseWorkTasksListRef");

const { items, pagination, searchQuery, loadWorkTasks, loading, taskTypes, priorities } = useMyWorkTasks({
  pageSize: 20,
  sort: "name:desc",
});

const title = computed(() => t("MY_TASKS.PAGES.LIST.TITLE"));

// Lifecycle
onMounted(async () => {
  await loadWorkTasks();
});

const reload = () => {
  baseWorkTasksListRef.value?.reload();
};

const onItemClick = (item: WorkTask) => {
  baseWorkTasksListRef.value?.onItemClick(item);
};

// Expose methods for external API
exposeToChildren({
  reload,
  onItemClick,
});
</script>
