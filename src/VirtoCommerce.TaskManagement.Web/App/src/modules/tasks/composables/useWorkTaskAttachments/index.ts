import { useUser } from "@vc-shell/framework";
import { filter, uniqueId } from "lodash-es";
import { computed, ref, Ref } from "vue";
import { WorkTask, WorkTaskAttachment } from "../../../../api_client/virtocommerce.taskmanagement";

interface IUseWorkTaskAttachments {
  fileUploading: Ref<boolean>;
  uploadAttachments(files: FileList, workTask: Ref<WorkTask>): Promise<Array<WorkTaskAttachment>>;
  deleteAttachment(id: string, workTask: Ref<WorkTask>): void;
}

export default (): IUseWorkTaskAttachments => {
  const fileUploading = ref(false);
  const defaultAttachmentsFolder = "draft";

  const uploadAttachments = async (files: FileList, workTask: Ref<WorkTask>): Promise<Array<WorkTaskAttachment>> => {
    const attachments = [] as Array<WorkTaskAttachment>;
    try {
      fileUploading.value = true;
      for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append("file", files[i]);
        const result = await fetch(
          `/api/assets?folderUrl=/worktaskattachment/${workTask.value.id || defaultAttachmentsFolder}`,
          {
            method: "POST",
            body: formData,
          },
        );
        const response = await result.json();
        if (response?.length) {
          const attachment = new WorkTaskAttachment(response[0]);
          attachment.id = uniqueId("Draft");
          attachment.createdDate = new Date();
          workTask.value.attachments = workTask.value.attachments || [];
          workTask.value.attachments.push(attachment);
        }
      }
    } catch (e) {
      console.log(e);
    } finally {
      fileUploading.value = false;
    }
    return attachments;
  };

  const deleteAttachment = (id: string, workTask: Ref<WorkTask>) => {
    workTask.value.attachments = filter(workTask.value.attachments, function (a) {
      return !!id && a.id !== id;
    });
  };

  return {
    fileUploading: computed(() => fileUploading.value),
    uploadAttachments,
    deleteAttachment,
  };
};
