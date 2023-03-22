<template>
  <VcStatus v-bind="statusStyles[workTaskStatus]">
    {{ localizedStatus() }}
  </VcStatus>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  inheritAttrs: false,
});
</script>

<script lang="ts" setup>
import { useI18n, VcStatus } from "@vc-shell/framework";

export interface Props {
  workTaskStatus: string;
}

const { t } = useI18n();

const props = withDefaults(defineProps<Props>(), {
  workTaskStatus: "ToDo",
});

const localizedStatus = () => {
  let result = t("TASKS.STATUS.TODO");

  switch (props.workTaskStatus) {
    case "ToDo":
      result = t("TASKS.STATUS.TODO");
      break;
    case "Canceled":
      result = t("TASKS.STATUS.CANCELED");
      break;
    case 'Done':
      result = t("TASKS.STATUS.DONE");
      break;
  }
  return result;
};

const statusStyles = {
  ToDo: {
    outline: false,
    variant: "info",
  },
  Canceled: {
    outline: false,
    variant: "danger",
  },
  Done: {
    outline: false,
    variant: "success",
  }
};

</script>
