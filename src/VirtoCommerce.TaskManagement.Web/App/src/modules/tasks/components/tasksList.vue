<template>
  <VcBlade
    :title="props.title"
    :expanded="props.expanded"
    :closable="props.closable"
    width="70%"
    :toolbarItems="bladeToolbar"
    @close="$emit('page:close')"
  >
    <!-- Blade contents -->
    <VcTable
      class="grow basis-0"
      state-key="tasks_list"
      :expanded="expanded"
      :empty="empty"
      :loading="loading"
      :columns="columns"
      :items="workTasks"
      :totalCount="totalCount"
      :pages="pages"
      :sort="sort"
      :searchValue="searchValue"
      :activeFilterCount="activeFilterCount"
      :selectedItemId="selectedItemId"
      :currentPage="currentPage"
      @search:change="onSearchList"
      @paginationClick="onPaginationClick"
      @itemClick="onItemClick"
      @scroll:ptr="reload"
      @headerClick="onHeaderClick"
      @selectionChanged="onSelectionChanged"
    >
      <!-- Filters -->
      <template v-slot:filters="{ closePanel }">
        <h2 v-if="$isMobile.value">
          {{ $t("TASKS.PAGES.LIST.FILTERS.TITLE") }}
        </h2>
        <VcContainer no-padding>
          <VcRow>
            <VcCol class="tw-w-[180px] tw-p-2">
              <div
                class="tw-mb-4 tw-text-[#a1c0d4] tw-font-bold tw-text-[17px]"
              >
                {{ $t("TASKS.PAGES.LIST.FILTERS.PRIORITY") }}
              </div>
              <div>
                <VcCheckbox
                  class="tw-mb-2"
                  v-for="priority in WorkTaskPriority"
                  :key="priority"
                  :modelValue="filter['priority'] === priority"
                  @update:modelValue="
                    filter['priority'] = $event ? priority : undefined
                  "
                  >{{ priority }}
                </VcCheckbox>
              </div>
            </VcCol>
            <VcCol class="tw-w-[180px] tw-p-2">
              <div
                class="tw-mb-4 tw-text-[#a1c0d4] tw-font-bold tw-text-[17px]"
              >
                {{ $t("TASKS.PAGES.LIST.FILTERS.DUEDATE") }}
              </div>
              <div>
                <VcInput
                  :label="$t('TASKS.PAGES.LIST.FILTERS.STARTDATE')"
                  type="date"
                  class="tw-mb-3"
                  :modelValue="getFilterDate('startDate')"
                  @update:modelValue="(e: string) => setFilterDate('startDate', e)"
                  max="9999-12-31"
                ></VcInput>
                <VcInput
                  :label="$t('TASKS.PAGES.LIST.FILTERS.ENDDATE')"
                  type="date"
                  :modelValue="getFilterDate('endDate')"
                  @update:modelValue="(e: string) => setFilterDate('endDate', e)"
                  max="9999-12-31"
                ></VcInput>
              </div>
            </VcCol>
          </VcRow>
          <VcRow>
            <VcCol class="tw-p-2">
              <div class="tw-flex tw-justify-end">
                <vc-button
                  outline
                  class="tw-mr-4"
                  @click="resetFilters(closePanel)"
                  :disabled="applyFiltersReset"
                  >{{ $t("TASKS.PAGES.LIST.FILTERS.RESETFILTERS") }}</vc-button
                >
                <vc-button
                  @click="applyFilters(closePanel)"
                  :disabled="applyFiltersDisable"
                  >{{ $t("TASKS.PAGES.LIST.FILTERS.APPLY") }}</vc-button
                >
              </div>
            </VcCol>
          </VcRow>
        </VcContainer>
      </template>

      <!-- Not found template -->
      <template v-slot:notfound>
        <div
          class="tw-w-full tw-h-full tw-box-border tw-flex tw-flex-col tw-items-center tw-justify-center"
        >
          <img :src="emptyImage" />
          <div class="tw-m-4 tw-text-xl tw-font-medium">
            {{ $t("TASKS.PAGES.LIST.NOTFOUND.NOTASKS") }}
          </div>
          <vc-button @click="resetSearch">{{
            $t("TASKS.PAGES.LIST.NOTFOUND.RESET")
          }}</vc-button>
        </div>
      </template>

      <!-- Empty template -->
      <template v-slot:empty>
        <div
          class="tw-w-full tw-h-full tw-box-border tw-flex tw-flex-col tw-items-center tw-justify-center"
        >
          <img :src="emptyImage" />
          <div class="tw-m-4 tw-text-xl tw-font-medium">
            {{ $t("TASKS.PAGES.LIST.EMPTY.NOMYTASKS") }}
          </div>
        </div>
      </template>

      <!-- Override priority column template -->
      <template v-slot:item_priority="itemData">
        <TaskPriority :workTaskPriority="itemData.item.priority"></TaskPriority>
      </template>

      <!-- Override status column template -->
      <template v-slot:item_status="itemData">
        <TaskStatus
          :work-task-status="calculateStatus(itemData.item)"
        ></TaskStatus>
      </template>

      <!-- Override responsible column template -->
      <template v-slot:item_responsibleName="itemData">
        <img
          class="tw-w-5 tw-h-5 tw-rounded-full"
          :src="getContactIcon(itemData.item.responsibleId)"
          onerror="javascript:this.src='/assets/userpic.svg'"
        />
        <span class="tw-ml-1 tw-pt-0.5">{{
          itemData.item.responsibleName
        }}</span>
      </template>

      <template v-slot:mobile-item="itemData">
        <div class="tw-p-3">
          <div class="tw-w-full tw-flex tw-justify-evenly">
            <div class="tw-grow tw-basis-0">
              <div class="tw-font-bold tw-text-lg">
                {{ itemData.item.number }}
              </div>
              <VcHint class="tw-mt-1">{{ itemData.item.name }}</VcHint>
            </div>
            <div>
              <TaskStatus
                v-bind:active="itemData.item.isActive"
                v-bind:completed="itemData.item.completed"
              ></TaskStatus>
            </div>
          </div>
          <div>
            <div class="tw-mt-3 tw-w-full tw-flex tw-justify-between">
              <div
                class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-grow tw-basis-0 tw-mr-2"
              >
                <VcHint>{{
                  $t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY")
                }}</VcHint>
                <div
                  class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-mt-1"
                >
                  <TaskPriority
                    :workTaskPriority="itemData.item.priority"
                  ></TaskPriority>
                </div>
              </div>
              <div
                class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-grow tw-basis-0 tw-mr-2"
              >
                <VcHint>{{
                  $t("TASKS.PAGES.LIST.TABLE.HEADER.DUEDATE")
                }}</VcHint>
                <div
                  class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-mt-1"
                >
                  {{ moment(itemData.item.dueDate).format("DD MMM") }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </VcTable>
  </VcBlade>
</template>

<script lang="ts">
import {
  VcBlade,
  VcTable,
  VcContainer,
  VcRow,
  VcCol,
  VcCheckbox,
  VcInput,
  VcButton,
  VcHint,
  IBladeToolbar,
  ITableColumns,
  useFunctions,
  useI18n,
  useUser,
} from "@vc-shell/framework";
import moment from "moment";
import {
  WorkTask,
  WorkTaskPriority,
  WorkTaskSearchCriteria,
} from "../../../api_client/taskmanagement";
import TaskPriority from "../components/taskPriority.vue";
import TaskStatus from "../components/taskStatus.vue";
import {
  computed,
  defineComponent,
  onMounted,
  reactive,
  ref,
  watch,
} from "vue";
import { useWorkTasks } from "../composables";
import emptyImage from "/assets/empty.png";

export default defineComponent({
  inheritAttrs: false,
});
</script>

<script lang="ts" setup>
export interface Props {
  title: string;
  isCurrentUserList: boolean;
  onlyComplitedList: boolean;
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

const emit = defineEmits(["onItemClick", "newTaskClick"]);

const props = withDefaults(defineProps<Props>(), {
  title: undefined,
  isCurrentUserList: false,
  onlyComplitedList: false,
  expanded: true,
  closable: true,
  param: undefined,
});

const { workTasks, totalCount, pages, loading, currentPage, loadWorkTasks } =
  useWorkTasks();
const { debounce } = useFunctions();
const { t } = useI18n();
const { user } = useUser();
const filter = reactive({});
const appliedFilter = ref({});
const searchValue = ref();
const selectedItemId = ref();
const selectedOrdersIds = ref([]);
const sort = ref("createdDate:DESC");
const applyFiltersDisable = computed(() => {
  const activeFilters = Object.values(filter).filter((x) => x !== undefined);
  return !activeFilters.length;
});
const applyFiltersReset = computed(() => {
  const activeFilters = Object.values(appliedFilter.value).filter(
    (x) => x !== undefined
  );
  return !activeFilters.length;
});

onMounted(async () => {
  selectedItemId.value = props.param;
  await loadWorkTasks(getCriteria());
});

watch(sort, async (value) => {
  await loadWorkTasks(getCriteria());
});

const bladeToolbar = ref<IBladeToolbar[]>([]);

if (props.isCurrentUserList === false && props.onlyComplitedList === false) {
  bladeToolbar.value = [
    {
      title: t("TASKS.PAGES.LIST.TOOLBAR.REFRESH"),
      icon: "fas fa-sync-alt",
      async clickHandler() {
        await reload();
      },
    },
    {
      title: t("TASKS.PAGES.LIST.TOOLBAR.CREATE"),
      icon: "fas fa-plus",
      async clickHandler() {
        emit("newTaskClick");
      },
    },
  ];
} else {
  bladeToolbar.value = [
    {
      title: t("TASKS.PAGES.LIST.TOOLBAR.REFRESH"),
      icon: "fas fa-sync-alt",
      async clickHandler() {
        await reload();
      },
    },
  ];
}

const tableColumns = ref<ITableColumns[]>([
  {
    id: "number",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NUMBER")),
    width: 30,
    alwaysVisible: true,
  },
  {
    id: "name",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NAME")),
    width: 150,
    alwaysVisible: true,
  },
  {
    id: "responsibleName",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE")),
    width: 150,
    class: "tw-flex tw-py-5",
  },
  {
    id: "createdBy",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.REPORTER")),
    width: 80,
  },
  {
    id: "priority",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY")),
    width: 30,
    alwaysVisible: true,
  },
  {
    id: "status",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.STATUS")),
    width: 50,
    alwaysVisible: true,
  },
  {
    id: "dueDate",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.DUEDATE")),
    sortable: true,
    width: 50,
    format: "DD MMM",
    type: "date",
    alwaysVisible: true,
  },
]);

const empty = reactive({
  image: emptyImage,
  text: computed(() => t("ORDERS.PAGES.EMPTY")),
});

const onItemClick = (item: { id: string }) => {
  selectedItemId.value = item.id;
  emit("onItemClick", item);
};

const onPaginationClick = async (page: number) => {
  await loadWorkTasks(getCriteria((page - 1) * 20));
};

const onSearchList = debounce(async (keyword: string) => {
  searchValue.value = keyword;
  await loadWorkTasks(getCriteria());
}, 200);

const reload = async () => {
  await loadWorkTasks(getCriteria());
};

const onHeaderClick = (item: ITableColumns) => {
  const sortOptions = ["DESC", "ASC", ""];
  if (item.sortable) {
    if (sort.value.split(":")[0] === item.id) {
      const index = sortOptions.findIndex((x) => {
        const sorting = sort.value.split(":")[1];
        if (sorting) {
          return x === sorting;
        } else {
          return x === "";
        }
      });
      if (index !== -1) {
        const newSort = sortOptions[(index + 1) % sortOptions.length];
        if (newSort === "") {
          sort.value = `${item.id}`;
        } else {
          sort.value = `${item.id}:${newSort}`;
        }
      }
    } else {
      sort.value = `${item.id}:${sortOptions[0]}`;
    }
  }
};

const onSelectionChanged = (checkboxes: { [key: string]: boolean }) => {
  selectedOrdersIds.value = Object.entries(checkboxes)
    .filter(([id, isChecked]) => isChecked)
    .map(([id, isChecked]) => id);
};

const columns = computed(() => {
  if (props.expanded) {
    return tableColumns.value;
  } else {
    return tableColumns.value.filter((item) => item.alwaysVisible === true);
  }
});

function setFilterDate(key: string, value: string) {
  const date = moment(value).toDate();
  if (date instanceof Date && !isNaN(date.valueOf())) {
    filter[key] = date;
  } else {
    filter[key] = undefined;
  }
}

function getFilterDate(key: string) {
  const date = filter[key] as Date;
  if (date) {
    return moment(date).format("YYYY-MM-DD");
  }
  return undefined;
}

function getCriteria(skip?: number): WorkTaskSearchCriteria {
  const criteria = new WorkTaskSearchCriteria();

  criteria.sort = sort.value;
  criteria.take = 20;
  criteria.skip = skip;
  criteria.priority = filter["priority"];
  criteria.startDueDate = filter["startDate"];
  criteria.endDueDate = filter["endDate"];
  criteria.keyword = searchValue.value;
  criteria.isActive = !props.onlyComplitedList;
  if (props.isCurrentUserList === true) {
    criteria.responsibleIds = [user.value.id];
  }

  return criteria;
}

async function resetSearch() {
  searchValue.value = "";
  Object.keys(filter).forEach((key: string) => (filter[key] = undefined));
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {};
}
const activeFilterCount = computed(
  () => Object.values(appliedFilter.value).filter((item) => !!item).length
);
async function applyFilters(closePanel: () => void) {
  closePanel();
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {
    ...filter,
  };
}
async function resetFilters(closePanel: () => void) {
  closePanel();
  Object.keys(filter).forEach((key: string) => (filter[key] = undefined));
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {};
}

const getContactIcon = (id: string) => {
  return "/api/task-management/contact/icon/" + id;
};

const calculateStatus = (workTask: WorkTask | any) => {
  let result = "ToDo";
  if (workTask.isActive === true) {
    switch (workTask.completed) {
      case false:
      case true:
        result = "Canceled";
        break;
    }
  } else {
    switch (workTask.completed) {
      case null:
      case false:
        result = "Canceled";
        break;
      case true:
        result = "Done";
        break;
    }
  }
  return result;
};

defineExpose({
  reload,
});
</script>
