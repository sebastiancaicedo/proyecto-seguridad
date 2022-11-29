import httpClient from './httpClient';

export function getUsersTable() {
  return httpClient
    .get('/operations/users-table')
    .then(({ data: json }) => json)
    .catch((err) => {
      throw new Error(JSON.stringify(err));
    });
}

export function encryptFile(reqData) {
  return httpClient
    .post('/operations/encrypt-file', reqData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    .then(({ data: json }) => json)
    .catch((err) => {
      throw new Error(JSON.stringify(err));
    });
}
