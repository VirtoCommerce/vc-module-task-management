<template>
  <TaskPriority :task-priority="item" />
</template>

<script setup lang="ts">
import { WorkTaskPriority } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  item: WorkTaskPriority;
}>();
</script>
