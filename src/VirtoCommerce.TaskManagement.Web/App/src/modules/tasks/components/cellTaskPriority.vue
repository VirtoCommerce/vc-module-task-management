<template>
  <TaskPriority :task-priority="context.item?.priority!"></TaskPriority>
</template>

<script setup lang="ts">
import { IWorkTask } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  context: {
    item: IWorkTask;
  };
}>();
</script>
