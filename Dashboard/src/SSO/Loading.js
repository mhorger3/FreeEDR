import React, { Fragment } from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import { CircularProgress, Typography } from "@material-ui/core";

const styles = theme => ({
  progress: {
    margin: theme.spacing.unit * 2
  }
});

function Loading(props) {
  const { classes, message } = props;
  return (
    <Fragment>
      <CircularProgress className={classes.progress} />
      <Typography variant="button" color="inherit">
        {message}
      </Typography>
    </Fragment>
  );
}

Loading.propTypes = {
  classes: PropTypes.object.isRequired,
  message: PropTypes.string.isRequired
};

export default withStyles(styles)(Loading);