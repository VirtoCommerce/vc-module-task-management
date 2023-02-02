<template>
  <VcStatus v-bind="statusStyle()">
    {{ status() }}
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
  active: boolean;
  completed: boolean | null;
}

const { t } = useI18n();

const props = withDefaults(defineProps<Props>(), {
  active: false,
  completed: null,
});

const status = () => {
  let result = t("TASKS.STATUS.TODO");

  if (props.active === true) {
    switch (props.completed) {
      case null:
        result = t("TASKS.STATUS.TODO");
        break;
      case false:
      case true:
        result = t("TASKS.STATUS.CANCELED");
        break;
    }
  } else {
    switch (props.completed) {
      case null:
      case false:
        result = t("TASKS.STATUS.CANCELED");
        break;
      case true:
        result = t("TASKS.STATUS.DONE");
        break;
    }
  }
  return result;
};

const statusStyle = () => {
  const result = {
    outline: true,
    variant: "info",
  };

  if (props.active === true) {
    result.outline = false;

    switch (props.completed) {
      case null:
        result.variant = "info";
        break;
      case false:
      case true:
        result.variant = "danger";
        break;
    }
  } else {
    result.outline = false;

    switch (props.completed) {
      case null:
      case false:
        result.variant = "danger";
        break;
      case true:
        result.variant = "success";
        break;
    }
  }
  return result;
};
</script>
