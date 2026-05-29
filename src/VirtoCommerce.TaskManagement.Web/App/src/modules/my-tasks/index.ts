import { defineAppModule } from "@vc-shell/framework";
import * as locales from "./locales";
import * as pages from "./pages";

export default defineAppModule({
  blades: pages,
  locales,
});
