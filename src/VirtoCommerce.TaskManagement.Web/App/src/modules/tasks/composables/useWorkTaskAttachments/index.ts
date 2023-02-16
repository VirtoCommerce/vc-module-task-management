import { useUser } from "@vc-shell/framework";
import { uniqueId } from "lodash-es";
import { computed, ref, Ref } from "vue";
import { WorkTaskAttachment } from "../../../../api_client/taskmanagement";

interface IUseWorkTaskAttachments {
  fileUploading: Ref<boolean>;
  uploadAttachments(
    id: string,
    files: FileList
  ): Promise<Array<WorkTaskAttachment>>;
}

export default (): IUseWorkTaskAttachments => {
  const fileUploading = ref(false);
  const { getAccessToken } = useUser();
  const defaultAttachmentsFolder = "Draft";

  const uploadAttachments = async (
    id: string,
    files: FileList
  ): Promise<Array<WorkTaskAttachment>> => {
    const attachments = [] as Array<WorkTaskAttachment>;
    try {
      fileUploading.value = true;
      for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append("file", files[i]);
        const authToken = await getAccessToken();
        const result = await fetch(
          `/api/assets?folderUrl=/workTaskAttachment/${
            id || defaultAttachmentsFolder
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
          attachment.id = uniqueId("Draft");
          attachment.createdDate = new Date();
          attachments.push(attachment);
        }
      }
    } catch (e) {
      console.log(e);
    } finally {
      fileUploading.value = false;
    }
    files = null;
    return attachments;
  };

  return {
    fileUploading: computed(() => fileUploading.value),
    uploadAttachments,
  };
};
