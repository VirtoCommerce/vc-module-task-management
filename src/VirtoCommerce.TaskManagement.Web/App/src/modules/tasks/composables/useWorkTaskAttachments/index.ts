import { useUser } from "@vc-shell/framework";
import { uniqueId, filter } from "lodash-es";
import { computed, ref, Ref } from "vue";
import {
  WorkTask,
  WorkTaskAttachment,
} from "../../../../api_client/taskmanagement";

interface IUseWorkTaskAttachments {
  fileUploading: Ref<boolean>;
  addAttachments(files: FileList, workTask: WorkTask);
  deleteAttachment(id: string, workTask: WorkTask);
}

export default (): IUseWorkTaskAttachments => {
  const fileUploading = ref(false);
  const { getAccessToken } = useUser();
  const defaultAttachmentsFolder = "Draft";

  const addAttachments = async (files: FileList, workTask: WorkTask) => {
    try {
      fileUploading.value = true;
      for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append("file", files[i]);
        const authToken = await getAccessToken();
        const result = await fetch(
          `/api/assets?folderUrl=/workTaskAttachment/${
            workTask.id || defaultAttachmentsFolder
          }`,
          {
            method: "POST",
            body: formData,
            headers: {
              Authorization: `Bearer ${authToken}`,
            },
          }
        );
        const response = await result.json();
        if (response?.length) {
          const attachment = new WorkTaskAttachment(response[0]);
          attachment.id = uniqueId(defaultAttachmentsFolder);
          attachment.createdDate = new Date();
          workTask.attachments.push(attachment);
        }
      }
    } catch (e) {
      console.log(e);
    } finally {
      fileUploading.value = false;
    }
    files = null;
  };

  const deleteAttachment = (id: string, workTask: WorkTask) => {
    workTask.attachments = filter(workTask.attachments, function (a) {
      if (id) {
        return a.id !== id;
      }
    });
  };

  return {
    fileUploading: computed(() => fileUploading.value),
    addAttachments,
    deleteAttachment,
  };
};
