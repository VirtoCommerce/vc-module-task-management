<template>
  <VcBlade
    v-loading="loading"
    :title="workTask.description"
    :expanded="expanded"
    :closable="closable"
    width="70%"
    :toolbarItems="bladeToolbar"
    @close="$emit('close:blade')"
  >
    <VcContainer>
      <VcRow>
        <VcCol size="1" class="p-2">
          <VcCard :header="$t('TASKS.PAGES.DETAILS.TASK_INFO.TITLE')">
            <VcRow class="p-2">
              <VcCol class="p-2">
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.DESCRIPTION')"
                  :value="workTask.description"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.CREATED_DATE')"
                  :value="createdDate"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.STORE')"
                  :value="workTask.storeId"
                />
                <div class="mb-[11px] last-of-type:mb-0">
                  <VcRow>
                    <VcCol>
                      <VcLabel>
                        <span>{{
                          $t("TASKS.PAGES.DETAILS.TASK_INFO.STATUS")
                        }}</span>
                      </VcLabel>
                    </VcCol>
                    <VcCol size="2">
                      <VcStatus v-bind="statusStyle(workTask)">
                        {{ status(workTask) }}
                      </VcStatus>
                    </VcCol>
                  </VcRow>
                </div>
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.WORKFLOW_ID')"
                  :value="workTask.workflowId"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.RESPONSIBLE_NAME')"
                  :value="workTask.responsibleName"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.PRIORITY')"
                  :value="workTask.priority"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.OBJECT_ID')"
                  :value="workTask.objectId"
                />
                <VcInfoRow
                  :label="$t('TASKS.PAGES.DETAILS.TASK_INFO.OBJECT_TYPE')"
                  :value="getParameterWidgetTitle()"
                />
              </VcCol>
            </VcRow>
          </VcCard>
        </VcCol>
      </VcRow>

      <VcWidget
        v-if="workTask.objectType"
        icon="fas fa-file-alt"
        :title="getParameterWidgetTitle()"
        class="border"
      >
      </VcWidget>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts">
import { defineComponent, computed, onMounted, ref } from "vue";
export default defineComponent({
  url: "task",
});
</script>

<script lang="ts" setup>
import moment from "moment";

import { useWorkTask } from "../composables";
import { useI18n, IBladeToolbar, IParentCallArgs } from "@vc-shell/framework";
import * as _ from "lodash-es";

import { WorkTask } from "../../../api_client/taskmanagement";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
}

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});

const emit = defineEmits<Emits>();

const { t } = useI18n();
const { workTask, loading, loadWorkTask, approveWorkTask, rejectWorkTask } =
  useWorkTask();
const locale = window.navigator.language;
const createdDate = computed(() => {
  const date = new Date(workTask.value?.createdDate);
  return moment(date).locale(locale).format("L LT");
});
let isParametersWidgetOpened = false;

onMounted(async () => {
  if (props.param) {
    await loadWorkTask(props.param);
  }
});

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.ACCEPT_TASK")),
    icon: "fas fa-check",
    async clickHandler() {
      if (props.param) {
        await approveWorkTask(props.param, workTask.value.parameters);
      }
    },
    disabled: computed(() => workTask.value.isActive === false),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.REJECT_TASK")),
    icon: "fas fa-ban",
    async clickHandler() {
      if (props.param) {
        await rejectWorkTask(props.param, workTask.value.parameters);
      }
    },
    disabled: computed(() => workTask.value.isActive === false),
  },
]);

const status = (workTask: WorkTask) => {
  let result = "Processing";

  if (workTask.isActive === true) {
    switch (workTask.completed) {
      case null:
        result = "Processing";
        break;
      case false:
      case true:
        result = "Rejected";
        break;
    }
  } else {
    switch (workTask.completed) {
      case null:
      case false:
        result = "Rejected";
        break;
      case true:
        result = "Success";
        break;
    }
  }
  return result;
};

const statusStyle = (workTask: WorkTask) => {
  const result = {
    outline: true,
    variant: "info",
  };

  if (workTask.isActive === true) {
    result.outline = false;

    switch (workTask.completed) {
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

    switch (workTask.completed) {
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

const getParameterWidgetTitle = () => {
  let result = "";
  if (workTask.value?.objectType) {
    result = _.last(workTask.value.objectType.split("."));
  }

  return result;
};

// async function openParameters() {
//   if (!isParametersWidgetOpened) {
//     emit("page:open", {
//       component: OffersList,
//       componentOptions: {
//         parameters: workTask.value.parameters,
//       },
//       url: null,
//       onOpen() {
//         isParametersWidgetOpened = true;
//       },
//       onClose() {
//         isParametersWidgetOpened = false;
//       },
//     });
//   }
// }
</script>
