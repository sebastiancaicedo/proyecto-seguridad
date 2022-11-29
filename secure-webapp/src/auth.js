export function setSession(token = '') {
  localStorage.setItem('token', token);
}

export function getSession() {
  return localStorage.getItem('token');
}

export function clearSession() {
  localStorage.clear();
}
