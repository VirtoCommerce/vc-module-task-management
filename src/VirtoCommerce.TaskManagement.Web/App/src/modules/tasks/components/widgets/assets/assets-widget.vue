<template>
  <VcWidget
    v-bind="props"
    :value="count"
    :title="$t('TASKS.PAGES.DETAILS.WIDGETS.ASSETS')"
    icon="far fa-file"
    @click="clickHandler"
  >
  </VcWidget>
</template>

<script setup lang="ts">
import { VcWidget, useBladeNavigation, usePopup, useAssets, useUser } from "@vc-shell/framework";
import { UnwrapNestedRefs, computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { isEqual } from "lodash-es";

import { useWorkTask } from "../../../composables";
import { WorkTaskAttachment } from "../../../../../api_client/virtocommerce.taskmanagement";

const props = defineProps<{
  // TODO Add to documentation
  modelValue: UnwrapNestedRefs<ReturnType<typeof useWorkTask>>;
}>();

const emit = defineEmits<{
  (event: "update:modelValue", context: UnwrapNestedRefs<ReturnType<typeof useWorkTask>>): void;
}>();

const { openBlade, resolveBladeByName } = useBladeNavigation();
const { showConfirmation } = usePopup();
const { t } = useI18n({ useScope: "global" });
const { edit, upload, remove, loading } = useAssets();
const modelValue = ref(props.modelValue);
const widgetOpened = ref(false);
const internalModel = ref();
const count = computed(() => modelValue.value?.item?.attachments?.length || 0);

watch(
  () => props.modelValue,
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
        assets: modelValue.value?.item?.attachments,
        loading: assetsHandler?.loading,
        assetsEditHandler: assetsHandler?.edit,
        assetsUploadHandler: assetsHandler?.upload,
        assetsRemoveHandler: assetsHandler?.remove,
        disabled: !props.modelValue.item?.isActive,
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
    internalModel.value.item.attachments = edit(files, internalModel.value.item.attachments);
    emitAssets();
    return internalModel.value.item.attachments;
  },
  async upload(files: FileList | null, lastSortOrder?: number) {
    if (files) {
      const uploaded = (await upload(files, `tasks/${internalModel.value.item.id}`, lastSortOrder)).map(
        (x) => new WorkTaskAttachment(x),
      );

      if (!internalModel.value.item.attachments) {
        internalModel.value.item.attachments = [];
      }

      internalModel.value.item.attachments = internalModel.value.item.attachments.concat(uploaded);

      files = null;

      emitAssets();
      return internalModel.value.item.attachments;
    }
  },
  async remove(files: WorkTaskAttachment[]) {
    if (
      await showConfirmation(
        computed(() => t("TASKS.PAGES.DETAILS.ALERTS.DELETE_CONFIRMATION_ASSET", { count: files.length })),
      )
    ) {
      internalModel.value.item.attachments = (await remove(files, internalModel.value.item.attachments)).map(
        (x) => new WorkTaskAttachment(x),
      );
    }
    emitAssets();
    return internalModel.value.item.attachments;
  },
};

function emitAssets() {
  emit("update:modelValue", internalModel.value);
}
</script>
