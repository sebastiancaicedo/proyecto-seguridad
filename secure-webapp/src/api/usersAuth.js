import httpClient from './httpClient';

export async function signUp({ name, lastName, userName, password }) {
  return httpClient
    .post(`/auth/signup`, { name, lastName, userName, password })
    .catch((err) => {
      throw new Error(JSON.stringify(err));
    });
}

export async function signIn({ userName, password }) {
  return httpClient
    .post(`/auth`, { userName, password })
    .then(({ data: json }) => json)
    .catch((err) => {
      throw new Error(JSON.stringify(err));
    });
}
