import { createDynamicAppModule } from "@vc-shell/framework";
import { schema, locales as baseLocales, components as moduleComponents } from "./../tasks";

import * as composables from "./composables";
import * as locales from "./locales";
import overrides from "./schema-overrides";

export default createDynamicAppModule({
  schema: {
    workTasksList: schema.workTasksList,
  },
  locales: {
    ...baseLocales,
    ...locales,
  },
  overrides,
  composables: {
    useWorkTasks: composables.useMyArchiveWorkTasks,
  },
  moduleComponents,
});
