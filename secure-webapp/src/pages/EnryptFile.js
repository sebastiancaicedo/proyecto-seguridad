import { Box, Button, TextField } from '@mui/material';
import { encryptFile } from '../api/operations';

export default function EncryptFile() {
  async function handleSubmit(evt) {
    evt.preventDefault();

    const data = new FormData(evt.currentTarget);
    const info = {
      file: data.get('file'),
      key: data.get('key'),
    };

    if (!validateFields(info)) {
      alert('invalid fields');
    }

    try {
      const res = await encryptFile(data);

      downloadFile(res.encryptedFile);
    } catch (error) {
      alert('An error occurred. Try again later');
    }
  }

  const validateFields = (fields) => {
    if (!fields.key || !fields.file) return false;

    return true;
  };

  const downloadFile = (content = 'nocontent') => {
    const link = document.createElement('a');
    const file = new Blob([content], { type: 'text/plain' });
    link.href = URL.createObjectURL(file);
    link.download = 'EncryptedFile.txt';
    link.click();
    URL.revokeObjectURL(link.href);
  };

  return (
    <main>
      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{
          m: 4,
          display: 'flex',
          flexWrap: 'wrap',
          justifyContent: 'space-between',
        }}
      >
        <TextField
          type="file"
          name="file"
          label="File to Encrypt"
          color="secondary"
          focused
          sx={{
            mt: 2,
            mb: 2,
          }}
        />
        <TextField
          type={'text'}
          name="key"
          label="Encryption Key"
          color="secondary"
          focused
          sx={{
            mt: 2,
            mb: 2,
          }}
        />
        <Button
          type="submit"
          color="secondary"
          variant="contained"
          sx={{
            mt: 2,
            mb: 2,
          }}
        >
          Encrypt
        </Button>
      </Box>
    </main>
  );
}
