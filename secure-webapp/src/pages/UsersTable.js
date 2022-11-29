import { Button } from '@mui/material';
import CloudDownloadIcon from '@mui/icons-material/CloudDownload';
import { getUsersTable } from '../api/operations';

export default function UsersTable() {
  async function onDownloadClick(evt) {
    try {
      const res = await getUsersTable();
      downloadFile(res.csv);
    } catch (error) {
      alert('An error occurred, try again later.');
    }
  }

  const downloadFile = (content = 'nocontent') => {
    const link = document.createElement('a');
    const file = new Blob([content], { type: 'text/plain' });
    link.href = URL.createObjectURL(file);
    link.download = 'UsersTable.csv';
    link.click();
    URL.revokeObjectURL(link.href);
  };

  return (
    <main>
      <Button
        onClick={onDownloadClick}
        variant="contained"
        color="secondary"
        sx={{
          m: 4,
        }}
        endIcon={<CloudDownloadIcon />}
      >
        Download Users Table
      </Button>
    </main>
  );
}
