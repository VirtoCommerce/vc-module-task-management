<template>
  <VcWidget
    v-bind="props"
    :value="count"
    :title="$t('TASKS.PAGES.DETAILS.WIDGETS.ASSETS')"
    icon="lucide-file"
    @click="clickHandler"
  >
  </VcWidget>
</template>

<script setup lang="ts">
import { useBlade, usePopup, useAssetsManager } from "@vc-shell/framework";
import { computed, markRaw, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { isEqual } from "lodash-es";
import { WorkTask, WorkTaskAttachment } from "../../../../../api_client/virtocommerce.taskmanagement";

import { VcWidget } from "@vc-shell/framework/ui";

const props = defineProps<{
  item: WorkTask;
}>();

const emit = defineEmits<{
  (event: "update:item", item: WorkTask): void;
}>();

const { openBlade } = useBlade();
const { showConfirmation } = usePopup();
const { t } = useI18n({ useScope: "global" });
const widgetOpened = ref(false);
const internalModel = ref<WorkTask>();
const count = computed(() => internalModel.value?.attachments?.length || 0);

const attachments = computed<WorkTaskAttachment[]>({
  get: () => internalModel.value?.attachments ?? [],
  set: (value) => {
    if (internalModel.value) {
      internalModel.value.attachments = value;
      emitAssets();
    }
  },
});

const assetsManager = useAssetsManager(attachments, {
  uploadPath: () => `tasks/${internalModel.value?.id}`,
  confirmRemove: () => showConfirmation(computed(() => t("TASKS.PAGES.ALERTS.DELETE_CONFIRMATION_ASSET"))),
});

watch(
  () => props.item,
  (newVal) => {
    if (!isEqual(internalModel.value, newVal)) {
      internalModel.value = newVal;
    }
  },
  { deep: true, immediate: true },
);

function clickHandler() {
  if (!widgetOpened.value) {
    openBlade({
      name: "AssetsManager",
      options: {
        manager: markRaw(assetsManager),
        disabled: !props.item?.isActive,
      },
      onOpen() {
        widgetOpened.value = true;
      },
      onClose() {
        widgetOpened.value = false;
      },
    });
  }
}

function emitAssets() {
  emit("update:item", internalModel.value as WorkTask);
}
</script>
