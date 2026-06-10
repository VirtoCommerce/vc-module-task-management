<template>
  <TaskPriority :task-priority="priority" />
</template>

<script setup lang="ts">
import { TaskPriority as TaskPriorityEnum } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  priority?: TaskPriorityEnum;
}>();
</script>
