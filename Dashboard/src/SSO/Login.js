import React from "react";
import { Route } from "react-router-dom";

import { authenticateUser } from "./authenticationService.js";

const Login = () => (
  <Route
    render={() => {
      authenticateUser();
      return null;
    }}
  />
);

export default Login;