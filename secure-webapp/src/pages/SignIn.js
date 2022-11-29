import * as React from 'react';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { Link, useNavigate } from 'react-router-dom';
import { signIn } from '../api/usersAuth';
import { clearSession, setSession } from '../auth';
import UserContext from '../containers/UserContext';

export default function SignIn() {
  const navigate = useNavigate();
  const { user, setUser } = React.useContext(UserContext);

  const handleSubmit = async (event) => {
    event.preventDefault();

    const data = new FormData(event.currentTarget);
    const creds = {
      userName: data.get('userName')?.trim(),
      password: data.get('password')?.trim(),
    };

    if (!validateFields(creds)) {
      alert('Invalid fields');
      return;
    }

    try {
      const res = await signIn(creds);
      setSession(res.token);
      setUser(res.user);
      navigate(`/`);
    } catch (error) {
      alert(
        'An error occurred, Maybe your credentials are wrong, try again later.'
      );
      clearSession();
      navigate('/signin');
    }
  };

  React.useEffect(function () {
    if (!!user) {
      navigate('/');
    }
  });

  const validateFields = (fields) => {
    var rgx =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!fields.userName || !fields.password) return false;
    if (rgx.test(fields.password) === false) return false;

    return true;
  };

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign in
        </Typography>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="userName"
            label="UserName"
            name="userName"
            autoFocus
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Sign In
          </Button>
          <Grid container>
            <Grid item>
              <Link to={'/signup'} variant="body2">
                {"Don't have an account? Sign Up"}
              </Link>
            </Grid>
          </Grid>
        </Box>
      </Box>
    </Container>
  );
}
