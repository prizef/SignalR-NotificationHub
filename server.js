import * as axios from "axios";

const URL_PREFIX = "";

export function notifications_delete(id) {
  return axios.delete(URL_PREFIX + "/api/notifications/" + id);
}