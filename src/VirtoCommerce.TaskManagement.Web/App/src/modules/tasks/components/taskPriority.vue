<template>
  <VcIcon icon="fas fa-circle" :class="getClass()"></VcIcon>
  <span class="tw-ml-1">{{ props.workTaskPriority }}</span>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  inheritAttrs: false,
});
</script>

<script lang="ts" setup>
import { VcIcon } from "@vc-shell/framework";
import { WorkTaskPriority } from "../../../api_client/taskmanagement";

export interface Props {
  workTaskPriority: WorkTaskPriority;
}

const props = withDefaults(defineProps<Props>(), {
  workTaskPriority: undefined,
});

const getClass = () => {
  return {
    "tw-text-[10px]": true,
    "tw-text-[#80b4e3]":
      props.workTaskPriority === WorkTaskPriority.Low ||
      props.workTaskPriority === WorkTaskPriority.Lowest,
    "tw-text-[#87b563]": props.workTaskPriority === WorkTaskPriority.Normal,
    "tw-text-[#eb4f4d]":
      props.workTaskPriority === WorkTaskPriority.High ||
      props.workTaskPriority === WorkTaskPriority.Highest,
  };
};
</script>
