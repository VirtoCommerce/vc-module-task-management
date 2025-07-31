<template>
  <BaseWorkTasksList
    v-bind="$props"
    ref="baseWorkTasksListRef"
    :title="title"
    state-key="my_archive_tasks_list"
    :items="items"
    :total-count="totalCount"
    :pages="pages"
    :current-page="currentPage"
    :loading="loading"
    :task-types="taskTypes"
    :priorities="priorities"
    :search-query="searchQuery"
    :load-work-tasks="loadWorkTasks"
    :show-create-button="false"
    @parent:call="$emit('parent:call', $event)"
    @close:blade="$emit('close:blade')"
    @collapse:blade="$emit('collapse:blade')"
    @expand:blade="$emit('expand:blade')"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, useTemplateRef } from "vue";
import { useI18n } from "vue-i18n";
import { IParentCallArgs } from "@vc-shell/framework";
import { useMyArchiveWorkTasks } from "../composables/useMyArchiveWorkTasks";
import { BaseWorkTasksList } from "../../tasks/components";
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
  name: "MyArchiveTasksList",
  url: "/my-archive",
  isWorkspace: true,
  menuItem: {
    title: "TASKS.MENU.MY_ARCHIVE_TITLE",
    icon: "material-description",
    priority: 1,
  },
});

const { t } = useI18n({ useScope: "global" });

const baseWorkTasksListRef = useTemplateRef("baseWorkTasksListRef");

const { items, totalCount, pages, currentPage, searchQuery, loadWorkTasks, loading, taskTypes, priorities } =
  useMyArchiveWorkTasks({
    pageSize: 20,
    sort: "name:desc",
  });

const title = computed(() => t("TASKS.PAGES.LIST.MY_ARCHIVE_TITLE"));

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
defineExpose({
  title,
  reload,
  onItemClick,
});
</script>
