<template>
  <div class="tw-flex tw-flex-row tw-items-center">
    <VcIcon icon="lucide-circle" :class="{ 'tw-text-[10px]': true, 'tw-text-[#80b4e3]': [TaskPriorityEnum.Low, TaskPriorityEnum.Lowest].indexOf(taskPriority!) > -1,
        'tw-text-[#87b563]': taskPriority === TaskPriorityEnum.Normal,
        'tw-text-[#eb4f4d]': [TaskPriorityEnum.High, TaskPriorityEnum.Highest].indexOf(taskPriority!) > -1,
      }"
    ></VcIcon>
    <span class="tw-ml-1">{{ taskPriority }}</span>
  </div>
</template>

<script setup lang="ts">
import { TaskPriority as TaskPriorityEnum } from "../../../api_client/virtocommerce.taskmanagement";

import { VcIcon } from "@vc-shell/framework/ui";

defineProps<{
  taskPriority?: TaskPriorityEnum;
}>();
</script>
