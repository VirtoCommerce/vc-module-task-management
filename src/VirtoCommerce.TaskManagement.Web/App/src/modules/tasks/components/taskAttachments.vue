<template>
  <div class="tw-flex tw-flex-wrap tw-p-5">
    <div class="tw-flex tw-flex-wrap tw-flex-1">
      <div
        class="vc-file-upload vc-file-upload_gallery tw-relative tw-h-[155px] tw-box-border tw-border tw-border-dashed tw-border-[#c8dbea] tw-rounded-[6px] tw-p-4 tw-m-2 tw-flex tw-flex-col tw-items-center tw-justify-center"
        v-for="item in props.workTask.attachments"
        v-bind:key="item.id"
      >
        <i
          class="vc-icon vc-icon_s fa-solid fa-xmark tw-text-[#c8dbea] hover:tw-text-[color:var(--app-bar-button-color-hover)] delete-icon"
          @click="deleteAttachment(item.id)"
          v-if="workTask.isActive === true"
        ></i>
        <i class="vc-icon vc-icon_xxl fa-solid fa-file tw-text-[#c8dbea]"></i>
        <div
          class="tw-text-[#9db0be] tw-text-center tw-text-lg tw-leading-lg tw-mt-4"
        >
          <a class="vc-link attachment-wrap" :href="item.url" target="_blank">
            {{ item.name }}</a
          >
        </div>
      </div>
    </div>
  </div>
  <div class="tw-flex tw-flex-wrap tw-p-5">
    <VcFileUpload
      @upload="addAttachments($event)"
      v-if="workTask.isActive === true"
      class="tw-m-2"
    ></VcFileUpload>
    <VcLoading :active="props.fileUploading"></VcLoading>
  </div>
</template>

<script lang="ts">
import { WorkTask } from "../../../api_client/taskmanagement";
import { defineComponent } from "vue";
import { VcFileUpload, VcLoading } from "@vc-shell/framework";

export default defineComponent({
  inheritAttrs: false,
});
</script>
<script lang="ts" setup>
export interface Props {
  workTask: WorkTask;
  fileUploading: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  workTask: undefined,
  fileUploading: false,
});

const emit = defineEmits(["addAttachments", "deleteAttachment"]);

const addAttachments = async (files: FileList) => {
  emit("addAttachments", files);
};

const deleteAttachment = async (id: string) => {
  emit("deleteAttachment", id);
};
</script>

<style lang="scss">
.attachment-wrap {
  overflow-wrap: anywhere;
}
.delete-icon {
  position: absolute;
  top: 2px;
  right: 0px;
  width: 20px;
  height: 20px;
  line-height: 20px;
  text-align: center;
  border: 0;
  font-weight: bold;
  font-size: 20px;
  cursor: pointer;
}
</style>
