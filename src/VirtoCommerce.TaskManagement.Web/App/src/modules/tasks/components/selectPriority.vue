<template>
  <TaskPriority :task-priority="context.opt"></TaskPriority>
</template>

<script setup lang="ts">
import { WorkTaskPriority } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  context: {
    opt: WorkTaskPriority;
  };
}>();
</script>
