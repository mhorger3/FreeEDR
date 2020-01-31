import React from "react";
import { Redirect } from "react-router-dom";
import { getAuthorizationToken } from "./authenticationService.js";
import Loading from "./Loading";

class Callback extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: true
    };
  }

  componentDidMount() {
    const queryParams = this.props.history.location.search;
    console.log(queryParams);

    // parse the query parameters to get the code
    const searchParams = new URLSearchParams(queryParams);
    const authorizationCode = searchParams.get("code");

    getAuthorizationToken(authorizationCode)
      .then(response =>
        sessionStorage.setItem(
          process.env.REACT_APP_BENCHMARK_AUTH_TOKEN,
          response.data
        )
      )
      .catch(error => console.log(error))
      .finally(() => this.setState({ isLoading: false }));
  }

  render() {
    const { isLoading } = this.state;

    return isLoading ? (
      <Loading message="Logging in...please wait" />
    ) : (
      <Redirect to="/" />
    );
  }
}

export default Callback;