import { BASE_URL, getHeaders, check } from "./config";

const API_URL = `${BASE_URL}/Customer`;

// VIEW ALL (search by name is optional)
export async function getCustomers(search) {
  const query = search ? `?search=${encodeURIComponent(search)}` : "";
  const res = await fetch(`${API_URL}/GetAll${query}`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// VIEW ONE
export async function getCustomer(id) {
  const res = await fetch(`${API_URL}/Get/${id}`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// UPDATE (send the userId too, or the customer loses their login link)
export async function updateCustomer(customer) {
  const res = await fetch(`${API_URL}/Update`, {
    method: "PUT",
    headers: getHeaders(),
    body: JSON.stringify(customer),
  });
  await check(res);
  return res.json();
}

// ACTIVATE / DEACTIVATE (the API answers with plain text, not JSON)
export async function toggleCustomerActive(id) {
  const res = await fetch(`${API_URL}/ToggleActive/${id}`, {
    method: "PUT",
    headers: getHeaders(),
  });
  await check(res);
  return res.text();
}