<template>
  <TaskPriority :task-priority="priority" />
</template>

<script setup lang="ts">
import { WorkTaskPriority } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  priority?: WorkTaskPriority;
}>();
</script>
