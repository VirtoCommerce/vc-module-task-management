import { computed, markRaw, Ref, ComputedRef } from "vue";
import { useBladeWidgets, useBlade, useAssetsManager, usePopup } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { WorkTask, WorkTaskAttachment } from "../../../api_client/virtocommerce.taskmanagement";

export interface UseTasksWidgetsOptions {
  item: Ref<WorkTask>;
  isVisible: ComputedRef<boolean>;
}

export function useTasksWidgets(options: UseTasksWidgetsOptions) {
  const { openBlade } = useBlade();
  const { showConfirmation } = usePopup();
  const { t } = useI18n({ useScope: "global" });

  const attachments = computed<WorkTaskAttachment[]>({
    get: () => options.item.value?.attachments ?? [],
    set: (value) => {
      if (options.item.value) {
        options.item.value.attachments = value;
      }
    },
  });

  const assetsManager = useAssetsManager(attachments, {
    uploadPath: () => `tasks/${options.item.value?.id ?? "draft"}`,
    confirmRemove: () => showConfirmation(computed(() => t("TASKS.PAGES.ALERTS.DELETE_CONFIRMATION"))),
  });

  return useBladeWidgets([
    {
      id: "AssetsWidget",
      icon: "lucide-file",
      title: "TASKS.PAGES.DETAILS.WIDGETS.ASSETS",
      badge: computed(() => options.item.value?.attachments?.length ?? 0),
      isVisible: options.isVisible,
      onClick: () =>
        openBlade({
          name: "AssetsManager",
          options: {
            manager: markRaw(assetsManager),
            disabled: !options.item.value?.isActive,
          },
        }),
    },
  ]);
}
