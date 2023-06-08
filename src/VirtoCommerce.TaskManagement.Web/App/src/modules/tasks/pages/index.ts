import _WorkTasksList from "./work-tasks-list.vue";
import _MyWorkTasksList from "./my-work-tasks-list.vue";
import _MyArchiveTasksList from "./my-archive-tasks-list.vue";

import _WorkTaskDetails from "./work-task-details.vue";
import _ArchiveWorkTasksList from "./archive-work-tasks-list.vue";

export const WorkTasksList = _WorkTasksList as typeof _WorkTasksList;
export const MyWorkTasksList = _MyWorkTasksList as typeof _MyWorkTasksList;
export const MyArchiveTasksList =
  _MyArchiveTasksList as typeof _MyArchiveTasksList;
export const WorkTaskDetails = _WorkTaskDetails as typeof _WorkTaskDetails;
export const ArchiveWorkTasksList =
  _ArchiveWorkTasksList as typeof _ArchiveWorkTasksList;
