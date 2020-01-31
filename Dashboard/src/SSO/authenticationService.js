import jwt_decode from "jwt-decode";
import axios from "axios";

// function to check if the object is empty ({})
const isAnEmptyObject = obj =>
  Object.keys(obj).length === 0 && obj.constructor === Object;

// check if user is authenticated
export function isUserAuthenticated() {
  const authToken = sessionStorage.getItem(
    process.env.REACT_APP_BENCHMARK_AUTH_TOKEN
  );
  if (authToken === null || authToken === "") {
    return false;
  } else {
    return true;
  }
}

// sets the authorization token to sessionStorage
export async function getAuthorizationToken(authorizationCode) {
  const adfsClientID = process.env.REACT_APP_AUTH_CLIENT_ID;
  const adfsRedirectUrl = process.env.REACT_APP_AUTH_REDIRECT_URI;
  const oauthRequestData = {
    grant_type: "authorization_code",
    client_id: adfsClientID,
    redirect_uri: adfsRedirectUrl,
    code: authorizationCode
  };
  const url = `${process.env.REACT_APP_ADFS_WEB_PROXY}`;

  const options = {
    method: "POST",
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Headers": "Origin, Content-Type, Accept",
      "Access-Control-Allow-Methods": "GET, POST"
    },
    data: oauthRequestData,
    json: true,
    url
  };

  const tokenResponse = await axios(options);

  return tokenResponse.data;
}

// decode the authenticated token
export function getDecodedToken() {
  const authToken = sessionStorage.getItem(
    process.env.REACT_APP_BENCHMARK_AUTH_TOKEN
  );
  let decodedToken = {};
  if (isUserAuthenticated) {
    decodedToken = jwt_decode(authToken);
  }

  return decodedToken;
}

// clear the user token
export function signoutUser() {
  sessionStorage.removeItem(process.env.REACT_APP_BENCHMARK_AUTH_TOKEN);
}

// get the logged in user
export function getLoggedInUser() {
  const decodedToken = getDecodedToken();

  // if empty object
  if (isAnEmptyObject(decodedToken)) {
    return {};
  } else {
    //const userName = decodedToken.upn.substring(0, decodedToken.upn.lastIndexOf("@"));;
    const userName = decodedToken.unique_name;
    const email = decodedToken.email;

    return { userName, email };
  }
}

export function authenticateUser() {
  const url = `${
    process.env.REACT_APP_AUTH_URI
  }/authorize?response_type=code&client_id=${
    process.env.REACT_APP_AUTH_CLIENT_ID
  }&resource=${process.env.REACT_APP_AUTH_RESOURCE}&redirect_uri=${
    process.env.REACT_APP_AUTH_REDIRECT_URI
  }`;
  // redirect to oauth server
  window.location.href = url;
}