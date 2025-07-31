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
import { VcWidget, useBladeNavigation, usePopup, useAssets } from "@vc-shell/framework";
import { computed, ref, watch, unref } from "vue";
import { useI18n } from "vue-i18n";
import { isEqual } from "lodash-es";
import { WorkTask, WorkTaskAttachment } from "../../../../../api_client/virtocommerce.taskmanagement";

const props = defineProps<{
  item: WorkTask;
}>();

const emit = defineEmits<{
  (event: "update:item", item: WorkTask): void;
}>();

const { openBlade, resolveBladeByName } = useBladeNavigation();
const { showConfirmation } = usePopup();
const { t } = useI18n({ useScope: "global" });
const { edit, upload, remove, loading } = useAssets();
const widgetOpened = ref(false);
const internalModel = ref();
const count = computed(() => internalModel.value?.attachments?.length || 0);

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
      blade: resolveBladeByName("AssetsManager"),
      options: {
        assets: internalModel.value?.attachments,
        loading: assetsHandler?.loading,
        assetsEditHandler: assetsHandler?.edit,
        assetsUploadHandler: assetsHandler?.upload,
        assetsRemoveHandler: assetsHandler?.remove,
        disabled: !unref(props.item)?.isActive,
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

const assetsHandler = {
  loading: computed(() => loading.value),
  edit: (files: WorkTaskAttachment[]) => {
    internalModel.value.attachments = edit(files, internalModel.value.attachments);
    emitAssets();
    return internalModel.value.attachments;
  },
  async upload(files: FileList | null, lastSortOrder?: number) {
    if (files) {
      const uploaded = (await upload(files, `tasks/${internalModel.value.id}`, lastSortOrder)).map(
        (x) => new WorkTaskAttachment(x),
      );

      if (!internalModel.value.attachments) {
        internalModel.value.attachments = [];
      }

      internalModel.value.attachments = internalModel.value.attachments.concat(uploaded);

      files = null;

      emitAssets();
      return internalModel.value.attachments;
    }
  },
  async remove(files: WorkTaskAttachment[]) {
    if (
      await showConfirmation(computed(() => t("TASKS.PAGES.ALERTS.DELETE_CONFIRMATION_ASSET", { count: files.length })))
    ) {
      internalModel.value.attachments = (await remove(files, internalModel.value.attachments)).map(
        (x) => new WorkTaskAttachment(x),
      );
    }
    emitAssets();
    return internalModel.value.attachments;
  },
};

function emitAssets() {
  emit("update:item", internalModel.value);
}
</script>
